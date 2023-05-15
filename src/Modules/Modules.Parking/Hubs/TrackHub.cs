using Microsoft.AspNetCore.SignalR;
using Modules.Parking.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Parking.Hubs
{
    public class TrackHub : Hub
    {
        private readonly IVehicleCheckinRepository _repository;

        public TrackHub(IVehicleCheckinRepository repository)
        {
            this._repository = repository;
        }

        public async Task SendTracks()
        {
            //DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")
            //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            var checkins = _repository.GetListVehicleCheckin(null, null, null, null);

            await Clients.All.SendAsync("ReceivedTracks", checkins).ConfigureAwait(true);
        }
    }
}
