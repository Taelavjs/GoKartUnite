using GoKartUnite.Interfaces;

namespace GoKartUnite.Controllers
{
    public class GroupController
    {
        private readonly IGroupHandler _groups;

        public GroupController(IGroupHandler groups)
        {
            _groups = groups;
        }


        public async Task index()
        {

        }


    }
}
