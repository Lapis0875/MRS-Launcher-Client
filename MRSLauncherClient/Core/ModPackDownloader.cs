﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace MRSLauncherClient
{
    // 진행률 표시용 델리게이트
    public delegate void DownloadModFileChangedEventHandler(object sender, DownloadModFileChangedEventArgs e);

    public class ModPackDownloader
    {
        // 진행률 표시용 이벤트
        public event DownloadModFileChangedEventHandler DownloadModFileChanged;

        ModPack modPack;
        string RootPath = "";
        string[] NoUpdate;

        public ModPackDownloader(ModPack pack, string downloadPath, string[] noUpdate)
        {
            this.NoUpdate = noUpdate;

            if (NoUpdate == null)
                NoUpdate = new string[] { };

            this.RootPath = downloadPath;
            this.modPack = pack;
        }

        public void DownloadFiles() // 로컬에 없는 파일 혹은 해쉬 다른 파일 다운로드
        {
            var webDownload = new WebDownload();
            webDownload.DownloadProgressChangedEvent += WebDownload_DownloadProgressChangedEvent;

            int fileCount = modPack.ModFiles.Length;
            for (int i = 0; i < fileCount; i++)
            {
                var item = modPack.ModFiles[i];
                var filepath = RootPath + item.Path + item.FileName;

                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                if (!File.Exists(filepath) || !CheckHash(filepath, item.MD5))
                    webDownload.DownloadFile(item.Url, filepath);

                FireDownloadModFileChanged(fileCount, i + 1); // 이벤트 발생
            }
        }

        int nowValue, maxValue;

        private void WebDownload_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            int eValue = 0;
            eValue = nowValue + e.ProgressPercentage;

            DownloadModFileChanged?.Invoke(this, new DownloadModFileChangedEventArgs(maxValue,eValue));
        }

        private void FireDownloadModFileChanged(int max, int current)
        {
            nowValue = current * 100;
            maxValue = max * 100;
            DownloadModFileChanged?.Invoke(this, new DownloadModFileChangedEventArgs(max, current));
        }

        private bool CheckHash(string filepath, string compareHash)
        {
            var hash = HashedFileEqualityCompaser.GetFileHash(filepath);
            return hash == compareHash;
        }

        private string[] GetFiles(string path)
        {
            return GetFiles(path, new List<string>()).ToArray();
        }

        private List<string> GetFiles(string path, List<string> result)
        {
            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                       .Select(x => x.FullName)
                       .ToArray();

            result.AddRange(files);

            var dirs = dir.GetDirectories("*.*", SearchOption.TopDirectoryOnly)
                      .Select(x => x.Name)
                      .ToArray();

            foreach (var item in dirs)
            {
                var fullPath = path + "\\" + item;
                if (NoUpdate.Contains(item))
                    continue;

                GetFiles(fullPath, result);
            }

            return result;
        }
    }

    // (진행률 표시용) 이벤트데이터 클래스
    public class DownloadModFileChangedEventArgs : EventArgs
    {
        public DownloadModFileChangedEventArgs(int maxfiles, int currentfiles)
        {
            this.MaxFiles = maxfiles;
            this.CurrentFiles = currentfiles;
        }

        public int MaxFiles { get; private set; }
        public int CurrentFiles { get; private set; }
    }
}
