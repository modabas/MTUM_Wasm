namespace MTUM_Wasm.Server.Core.Common.Utility
{
    internal class Definitions
    {
        public const string UserAuthenticatedStreamNamespace = "User.OnAuthenticated";
        public const string MemoryStreamProviderName = "InMemoryStreamProvider";
        public const string StateStorageName = "StateStore";
        public const string PubSubStorageName = "PubSubStore";
        //this is a unique ID for the Orleans cluster. All clients and silos that use this ID will be able to talk directly to each other. You can choose to use a different ClusterId for different deployments, though.
        //for heteregenous cluster (cluster that constains silos that implement different kinds of grains), orleans will handle the communication with correct silos that support that kind of grain
        public const string SiloClusterId = "MTUM_Wasm";
        //this is a unique ID for your application that will be used by some providers, such as persistence providers.This ID should remain stable and not change across deployments.
        public const string SiloServiceId = "MTUM_Wasm.Server";


    }
}
