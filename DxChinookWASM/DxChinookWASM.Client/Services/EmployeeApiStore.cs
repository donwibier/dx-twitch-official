using DxChinook.Data.Models;

namespace DxChinookWASM.Client.Services
{
    public class EmployeeApiStore : ApiStore<int, EmployeeModel>
    {
        public EmployeeApiStore(HttpClient httpClient) : base(httpClient)
        {

        }

        public override string KeyField => nameof(EmployeeModel.EmployeeId);

        public override string ControllerBase => "api/Employees";

        public override int ModelKey(EmployeeModel model) => model.EmployeeId;

        public override void SetModelKey(EmployeeModel model, int key) => model.EmployeeId = key;

        
    }
}
