using DxChinook.Data.Models;
using DxChinookWASM.Client.Services;

namespace DxChinookv8.Client.Data
{
    public class ArtistApiStore : ApiStore<int, ArtistModel>
    {
        public ArtistApiStore(HttpClient httpClient) : base(httpClient)
        {

        }

        public override string KeyField => nameof(ArtistModel.ArtistId);

        public override string ControllerBase => "api/Artists";

        public override int ModelKey(ArtistModel model) => model.ArtistId;

        public override void SetModelKey(ArtistModel model, int key) => model.ArtistId = key;
    }
}
