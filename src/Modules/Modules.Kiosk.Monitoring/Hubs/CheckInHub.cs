using Microsoft.AspNetCore.SignalR;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using System;
using System.Threading.Tasks;

namespace Modules.Kiosk.Monitoring.Hubs
{
    public class CheckInHub : Hub
    {
        private readonly IKIOCheckInRepository _repository;

        public CheckInHub(IKIOCheckInRepository repository)
        {
            this._repository = repository;
        }

        public async Task SendCheckIns()
        {
            //DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")
            //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            var checkins = _repository.GetCheckInInfo(null, DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 60);

            await Clients.All.SendAsync("ReceivedCheckIns", checkins).ConfigureAwait(true);
        }

        public async Task SendDeploySuccess()
        {
            await Clients.All.SendAsync("SendDeployedSuccess").ConfigureAwait(true);
        }
    }
    
}
