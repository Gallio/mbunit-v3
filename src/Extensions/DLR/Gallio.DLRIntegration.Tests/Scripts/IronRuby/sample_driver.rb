# sample_driver.rb

require 'gallio'

class SampleDriver
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
    # Messages
    puts 'StdOut message'
    $stderr.puts 'StdErr message'
    
    # Logging
    @logger.error('Log error')
    @logger.warning('Log warning')
    @logger.important('Log important')
    @logger.info('Log info')
    @logger.debug('Log debug')
    
    # Verb
    puts "Verb #{@verb}"
    
    # Test Package
    @test_package.Files.each do |file| puts "File #{file.Name}" end
  end
end

# Run.

::SampleDriver.new(ScriptParameters).go
