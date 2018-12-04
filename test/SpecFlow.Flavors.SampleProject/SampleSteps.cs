using System;
using TechTalk.SpecFlow;

namespace SpecFlow.Flavors.SampleProject
{
    [Binding]
    public class SampleSteps
    {
	    public SampleSteps(ISampleDriver driver)
	    {
		    
	    }

        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int value)
        {
			throw new PendingStepException();
        }
        
        [When(@"I press add")]
        public void WhenIPressAdd()
		{
			throw new PendingStepException();
		}
        
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int value)
		{
			throw new PendingStepException();
		}
    }

	public interface ISampleDriver
	{
	}

	[Flavor("Vanilla")]
	public class VanillaDriver : ISampleDriver
	{
	}

	[Flavor("Chocolate")]
	public class ChocolateDriver : ISampleDriver
	{
		
	}
}
