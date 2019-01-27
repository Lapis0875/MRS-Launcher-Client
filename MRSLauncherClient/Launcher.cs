﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    class Launcher // 런처 전역에서 사용되는 각종 상수들을 모아둔 클래스
    {
        public static readonly string
            LauncherName = "MRSLauncher",
            LauncherVersion = "test1",

            LauncherPath = Environment.GetEnvironmentVariable("appdata"), // 모든 파일이 저장되는 기본 경로
            GamePath = LauncherPath + "\\games", // 마인크래프트가 설치되는 경로
            JavaPath = LauncherPath + "\\runtime", // 자바가 설치되는 경로

            ModPackListUrl = "https://api.mysticrs.tk/list", // 모드팩 리스트
            ModPackDataUrl = "https://api.mysticrs.tk/modpack"; // 모드팩 정보 (모드파일 등)
    }
}