namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using MediatR;

    public class JobRequest: IRequest<bool>
    {
        public string Name { get; set; }

        public string ApiUrl { get; set; }

        public JobRequest(string name, string apiUrl)
        {
            this.Name = name;
            this.ApiUrl = apiUrl;
        }
    }
}
