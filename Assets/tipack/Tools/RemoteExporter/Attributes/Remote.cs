using System;

/// <summary>
/// A custom attribute used to facilitate the detection of variables for remote connection.
/// </summary>
/// <param name="ignoreRemoteConnection">
/// <para> This parameter is used to determine whether the variable will be used in the connection test. </para>
/// <para> Set this true if you want to ignore in connection test. (Default = false).</para>
/// </param>
namespace tiplay.RemoteExporterTool
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class Remote : Attribute
    {
        public bool IgnoreRemoteConnection { get; }

        public Remote(bool ignoreRemoteConnection = false)
        {
            IgnoreRemoteConnection = ignoreRemoteConnection;
        }
    }
}