require 'bowling'

describe Bowling do

  it "should score 0 for gutter game" do
    bowling = Bowling.new

    20.times { bowling.hit(0) }

    bowling.score.should == 0
  end

  it "should score 300 for perfect game" do
    bowling = Bowling.new

    20.times { bowling.hit(10) }

    bowling.score.should == 300
  end

end