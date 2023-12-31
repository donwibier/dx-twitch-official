﻿using DxChinook.Data.Models;

namespace DxChinookWASM.Client.Services
{
    public class CustomerApiStore : ApiStore<int, CustomerModel>
    {
        public CustomerApiStore(HttpClient httpClient) : base(httpClient)
        {

        }

        public override string KeyField => nameof(CustomerModel.CustomerId);
        public override string ControllerBase => "api/Customers"; //"api/cust";
        public override int ModelKey(CustomerModel model) => model.CustomerId;
        public override void SetModelKey(CustomerModel model, int key) => model.CustomerId = key;
    }
}
