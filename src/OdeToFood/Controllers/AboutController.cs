using Microsoft.AspNet.Mvc;


namespace OdeToFood.Controllers
{
    //Attribute based routing
    [Route("[controller]")]
    public class AboutController
    {
        [Route("")]
        public string Phone()
        {
            return "+1-555-555-5555";
        }
        [Route("[action]")]
        public string Country()
        {
            return "USA";
        }
    }
}
