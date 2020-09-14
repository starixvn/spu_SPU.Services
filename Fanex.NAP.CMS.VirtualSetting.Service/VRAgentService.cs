namespace Fanex.NAP.CMS.VirtualSetting.Service
{
    using Fanex.NAP.CMS.VirtualSetting.DataAccess;
    using Fanex.NAP.CMS.VirtualSetting.Model;
    using Fanex.NAP.SPU.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VRAgentService 
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }        

        public VRAgentService(string siteIds, string superIds, DateTime adjustDate)
        {
            var model = new VirtualSetting
            {
                ServiceName = "VR_AgentAdjustment",
                AdjustDate = adjustDate,
                SportGroupID = 0,
                SiteIds = siteIds,
                SuperIds = superIds,
                FailedUpdatePunterList = new List<Punter>()
            };        

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess, true);
        }

        public void AutoVRAgentAdjustment()
        {
            _punterService.InsertNewVRAgent();

            var (updateFlagAbnormalAgents, updateUnflagAbnormalAgents) = _punterService.VerifySportsAgentPerformance();

            UpdateFlagAbnormalAgent(updateFlagAbnormalAgents, 1);

            UpdateUnflagAbnormalAgent(updateUnflagAbnormalAgents, 1);
        }

        private void UpdateFlagAbnormalAgent(IEnumerable<int> customerList, int type)
        {
            var customerListString = string.Join(",", customerList);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateFlagAbnormalAgent(string.Join(",", customerChunks[i]), type);
                PuntersUpdated += customerChunks[i].Count();
            }
        }

        private void UpdateUnflagAbnormalAgent(IEnumerable<int> customerList, int type)
        {
            var customerListString = string.Join(",", customerList);

            var customerChunks = Utilities.ChunkList(customerList, AppSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                _punterService.UpdateUnflagAbnormalAgent(string.Join(",", customerChunks[i]), type);
                PuntersUpdated += customerChunks[i].Count();
            }
        }

        public (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated(DateTime adjustDate)
        {
           return _punterService.GetVRAgentUpdated(adjustDate);
        }
    }
}
