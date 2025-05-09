﻿namespace WebAPI.Utilities.Contract;

public interface IResourceRight<out TResource>
{
    string ResourceRight { get; set; }

    TResource SetResourceRight(int? requesterId);
}

public enum ResourceRights
{
    Owner,
    Viewer
}