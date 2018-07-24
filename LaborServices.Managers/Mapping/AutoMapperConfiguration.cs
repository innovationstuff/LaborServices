using AutoMapper;
using LaborServices.Model;
using LaborServices.ViewModel;

namespace LaborServices.Managers.Mapping
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureMapping()
        {
            Mapper.Initialize(config =>
            {
                //config.CreateMap<Agent, AgentGeneralVM>();
            });
        }
    }
}
