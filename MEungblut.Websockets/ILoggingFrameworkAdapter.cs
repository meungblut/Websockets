﻿namespace MEungblut.Websockets
{
    using System;

    public interface ILoggingFrameworkAdapter
    {
        void LogException(Exception exception, params object[] objectsToLog);

        void LogInfo(string message, params object[] objectsToLog);
        void LogDebug(string message, params object[] objectsToLog);
        void LogWarning(string message, params object[] objectsToLog);
        void LogError(string message, params object[] objectsToLog);

    }
}