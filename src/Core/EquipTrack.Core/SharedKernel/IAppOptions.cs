namespace EquipTrack.Core.SharedKernel;

public interface IAppOptions
{
    /// <summary>
    /// The configuration section path.
    /// </summary>
    static abstract string ConfigSectionPath { get; }
}