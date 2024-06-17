using System;
using UnityEngine.Events;

namespace ET
{
public static class PathResouceEvent
{
    public static Action<PatchEventDefine.InitializeFailed> InitializeFailed;
    public static Action<PatchEventDefine.PatchStatesChange> PatchStatesChange;
    public static Action<PatchEventDefine.FoundUpdateFiles> FoundUpdateFiles;
    public static Action<PatchEventDefine.DownloadProgressUpdate> DownloadProgressUpdate;
    public static Action<PatchEventDefine.PackageVersionUpdateFailed> PackageVersionUpdateFailed;
    public static Action<PatchEventDefine.PatchManifestUpdateFailed> PatchManifestUpdateFailed;
    public static Action<PatchEventDefine.WebFileDownloadFailed> WebFileDownloadFailed;
}
public class PatchEventDefine
{
    /// <summary>
    /// 补丁包初始化失败
    /// </summary>
    public class InitializeFailed 
    {
        public static void SendEventMessage()
        {
            var msg = new InitializeFailed();
            PathResouceEvent.InitializeFailed?.Invoke(msg);
        }
    }

    /// <summary>
    /// 补丁流程步骤改变
    /// </summary>
    public class PatchStatesChange 
    {
        public string Tips;

        public static void SendEventMessage(string tips)
        {
            var msg = new PatchStatesChange();
            msg.Tips = tips;
            PathResouceEvent.PatchStatesChange?.Invoke(msg);
        }
    }

    /// <summary>
    /// 发现更新文件
    /// </summary>
    public class FoundUpdateFiles : UnityEvent
    {
        public int TotalCount;
        public long TotalSizeBytes;
        
        public static void SendEventMessage(int totalCount, long totalSizeBytes)
        {
            var msg = new FoundUpdateFiles();
            msg.TotalCount = totalCount;
            msg.TotalSizeBytes = totalSizeBytes;
            
            PathResouceEvent.FoundUpdateFiles?.Invoke(msg);
        }
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    public class DownloadProgressUpdate : UnityEvent
    {
        public int TotalDownloadCount;
        public int CurrentDownloadCount;
        public long TotalDownloadSizeBytes;
        public long CurrentDownloadSizeBytes;
        
        public static void SendEventMessage(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            var msg = new DownloadProgressUpdate();
            msg.TotalDownloadCount = totalDownloadCount;
            msg.CurrentDownloadCount = currentDownloadCount;
            msg.TotalDownloadSizeBytes = totalDownloadSizeBytes;
            msg.CurrentDownloadSizeBytes = currentDownloadSizeBytes; 
            PathResouceEvent.DownloadProgressUpdate?.Invoke(msg);
        }
    }

    /// <summary>
    /// 资源版本号更新失败
    /// </summary>
    public class PackageVersionUpdateFailed 
    {
        public static void SendEventMessage()
        {
            var msg = new PackageVersionUpdateFailed();
            PathResouceEvent.PackageVersionUpdateFailed?.Invoke(msg);
        }
    }

    /// <summary>
    /// 补丁清单更新失败
    /// </summary>
    public class PatchManifestUpdateFailed 
    {
        public static void SendEventMessage()
        {
            var msg = new PatchManifestUpdateFailed();
            PathResouceEvent.PatchManifestUpdateFailed?.Invoke(msg);
        }
    }

    /// <summary>
    /// 网络文件下载失败
    /// </summary>
    public class WebFileDownloadFailed 
    {
        public string FileName;
        public string Error;

        public static void SendEventMessage(string fileName, string error)
        {
            var msg = new WebFileDownloadFailed();
            msg.FileName = fileName;
            msg.Error = error;
            
            PathResouceEvent.WebFileDownloadFailed?.Invoke(msg);
        }
    }
}
}