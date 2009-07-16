# driver.rb
#
# Runs RSpec tests.
#
# This script receives inputs from Gallio in the form of a dictionary called ScriptParameters.
# See the class comments on DLRTestDriver for more information about these parameters.
#

require 'gallio'
require 'spec'
require 'spec/runner/formatter/base_formatter'
require 'shellwords'

module Gallio
  module RSpecAdapter
  
    class Driver
      def initialize(parameters)
        @verb = parameters['Verb']
        @test_package = parameters['TestPackage']
        @test_exploration_options = parameters['TestExplorationOptions']
        @text_execution_options = parameters['TestExecutionOptions']
        @message_sink = ::Gallio::Wrappers::MessageSink.new(parameters['MessageSink'])
        @progress_monitor = ::Gallio::Wrappers::ProgressMonitor.new(parameters['ProgressMonitor'])
        @logger = ::Gallio::Wrappers::Logger.new(parameters['Logger'])
      end
      
      def go
        @logger.debug('RSpec adapter started.')
      
        case @verb
          when 'Explore'
            explore
          
          when 'Run'
            run
        end
            
      ensure      
        @logger.debug('RSpec adapter finished.')        
      end
      
    private
      
      def explore
        explore_or_run(true)
      end
      
      def run
        explore_or_run(false)
      end
      
      def explore_or_run(exploring)
        options = create_options(exploring)
        options.formatters << Formatter.new(options, @message_sink, @progress_monitor)
        
        ::Spec::Runner.use options
        ::Spec::Runner.run
      end
      
      def create_options(exploring)
        options_str = @test_package.Properties.GetValue('RSpecOptions') || ''
        @logger.debug("RSpec options: '#{options_str}'")
        
        options_array = ::Shellwords::shellwords(options_str)
        options_array << '--dry-run' if exploring
        options_array << '--format' << 'silent'
        @test_package.Files.each do |file|
          options_array << file.FullName
        end
        
        workaround_option_parser_use_of_argument_zero        
        parser = ::Spec::Runner::OptionParser.new($stderr, $stdout)
        parser.parse(options_array)
        
        parser.options
      end
      
      # HACK: Work around some bad code in OptionParser.spec_command? where it
      #       tries to read $0.  Unfortunately $0 is nil here.
      def workaround_option_parser_use_of_argument_zero
        $0 = ''  
      end
      
      class Formatter < ::Spec::Runner::Formatter::BaseFormatter
        def initialize(options, message_sink, progress_monitor)
          @options = options
          @message_sink = message_sink
          @progress_monitor = progress_monitor
          
          test_context_tracker = ::Gallio::Wrappers::Runtime.Resolve(::Gallio::Model::Contexts::ITestContextTracker)
          @test_context_manager = ::Gallio::Model::Contexts::ObservableTestContextManager.new(test_context_tracker, message_sink.inner)
          @test_stack = []
          @test_context_stack = []
        end
        
        def start(example_count)
          #puts "start"
          @progress_monitor.begin_task(dry_run? ? "Exploring RSpec tests." : "Running RSpec tests.", example_count < 1 ? 1 : example_count)
          
          root_test = create_root_test
          @test_stack.push(root_test)
          
          notify_test_discovered(root_test)
        
          return if dry_run?
          
          root_test_step = create_test_step(root_test)
          root_test_context = @test_context_manager.StartStep(root_test_step)
          @test_context_stack.push(root_test_context)
        end

        def example_group_started(example_group_proxy)
          #puts "example_group_started"
          example_group_finished unless @test_stack.length < 2
        
          @progress_monitor.set_status("Running example group: #{example_group_proxy.description}.")
          
          example_group_test = create_test(example_group_proxy.description)
          example_group_test.Kind = "RSpecExampleGroup"
          @test_stack.last.AddChild(example_group_test)
          @test_stack.push(example_group_test)
          
          notify_test_discovered(example_group_test, example_group_proxy.location)
          
          return if dry_run?
          
          example_group_test_context_step = create_test_step(example_group_test, @test_context_stack.last.TestStep)
          example_group_test_context = @test_context_stack.last.StartChildStep(example_group_test_context_step)
          @test_context_stack.push(example_group_test_context)
        end
        
        def example_started(example_proxy)
          #puts "example_started"
          @progress_monitor.set_status("Running example: #{example_proxy.description.ToString()}.")
          
          example_test = create_test(example_proxy.description)
          example_test.Kind = "RSpecExample"
          example_test.IsTestCase = true
          @test_stack.last.AddChild(example_test)
          @test_stack.push(example_test)
          
          notify_test_discovered(example_test, example_proxy.location)
          
          return if dry_run?
          
          example_test_context_step = create_test_step(example_test, @test_context_stack.last.TestStep)
          example_test_context = @test_context_stack.last.StartChildStep(example_test_context_step)
          @test_context_stack.push(example_test_context)
        end

        def example_passed(example_proxy)
          #puts "example_passed"
          @progress_monitor.worked(1)
          @test_stack.pop()
          
          return if dry_run?
          
          example_test_context = @test_context_stack.pop()
          set_outcome(example_test_context, ::Gallio::Model::TestOutcome.Passed)
          finish_test_step(example_test_context)
        end

        def example_failed(example_proxy, counter, failure)
          #puts "example_failed"
          @progress_monitor.worked(1)
          @test_stack.pop()
          
          return if dry_run?
          
          example_test_context = @test_context_stack.pop()
          example_test_context.LogWriter.Warnings.BeginSection(failure.header)
          example_test_context.LogWriter.Warnings.Write(failure.exception.message) unless failure.exception.nil?
          example_test_context.LogWriter.Warnings.Write(format_backtrace(failure.exception.backtrace)) unless failure.exception.nil?
          example_test_context.LogWriter.Warnings.End()
          set_outcome(example_test_context, ::Gallio::Model::TestOutcome.Failed)
          finish_test_step(example_test_context, true)
        end
        
        def example_pending(example_proxy, message, deprecated_pending_location=nil)
          #puts "example_pending"
          @progress_monitor.worked(1)
          @test_stack.pop()
          
          return if dry_run?
          
          example_test_context = @test_context_stack.pop()
          example_test_context.AddMetadata(::Gallio::Model::MetadataKeys::PendingReason, message)
          example_test_context.LogWriter.Warnings.BeginSection("Pending")
          example_test_context.LogWriter.Warnings.Write(message)
          example_test_context.LogWriter.Warnings.End()
          set_outcome(example_test_context, ::Gallio::Model::TestOutcome.Pending)
          finish_test_step(example_test_context)
        end
        
        def start_dump
          #puts "start_dump"
          example_group_finished unless @test_stack.length < 2
          finished_run unless @test_stack.length < 1
        end

        def close
          @progress_monitor.done
        end
        
      private
      
        def example_group_finished
          @test_stack.pop()
          
          return if dry_run?
          
          example_group_test_context = @test_context_stack.pop()
          finish_test_step(example_group_test_context)
        end
        
        def finished_run
          if ! dry_run? && @test_stack.length != @test_context_stack.length
            puts "The test stack length is #{@test_stack.length} but the context stack length is #{test_context_stack.length}.  This usually indicates a fatal error occurred somewhere else inside the test adapter."
          end
        
          #puts "#{} #{}"
          @test_stack.pop()
          
          return if dry_run?
          
          root_test_context = @test_context_stack.pop()
          finish_test_step(root_test_context)
        end
        
        def set_outcome(test_context, outcome)
          test_context.SetInterimOutcome(outcome)
        end
        
        def finish_test_step(test_context, propagate_to_parent = false)
          outcome = test_context.Outcome
          test_context.FinishStep(outcome, nil)
          
          if propagate_to_parent
            parent_outcome = test_context.Parent.Outcome.CombineWith(outcome).Generalize()
            test_context.Parent.SetInterimOutcome(parent_outcome)
          end
        end
        
        def format_backtrace(backtrace)
          return "" if backtrace.nil?
          backtrace.map { |line| backtrace_line(line) }.join("\n")
        end

        def backtrace_line(line)
          line.sub(/\A([^:]+:\d+)$/, '\\1:')
        end
      
        def dry_run?
          @options.dry_run?
        end
        
        def create_root_test
          ::Gallio::Model::Tree::RootTest.new
        end
      
        def create_test(name)
          ::Gallio::Model::Tree::Test.new(name, nil)
        end
        
        def create_test_step(test, parent_test_step = nil)
          ::Gallio::Model::Tree::TestStep.new(test, parent_test_step)
        end
        
        def notify_test_discovered(test, location = nil)
          message = ::Gallio::Model::Messages::Exploration::TestDiscoveredMessage.new
          message.ParentTestId = test.Parent.Id if test.Parent
          message.Test = ::Gallio::Model::Schema::TestData.new(test)
          
          if location != nil
            matches = /^(.*):([0-9]+)$/.match(location)
            if matches != nil
              path = matches[1]
              line = matches[2].to_i
            else
              path = location
              line = 0
            end
            
            message.Test.CodeLocation = ::Gallio::Common::Reflection::CodeLocation.new(path, line, 0)
          end
        
          @message_sink.publish(message)
        end        
      end
    end
  end
end

# Run.

::Gallio::RSpecAdapter::Driver.new(ScriptParameters).go
