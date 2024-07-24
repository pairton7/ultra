using UltraBank.Domain.ValueObjects;
using UltraBank.NotificationContext.Interfaces;

namespace UltraBank.WebApi.Controllers.ArchiveContext.Sendloads;

public readonly struct UploadArchiveSendloadOutput
{
    public Guid ArchiveId { get; }
    public string ArchiveType { get; }
    public long ArchiveSize { get; }
    public INotification[] Notifications { get; }

    private UploadArchiveSendloadOutput(Guid archiveId, string archiveType, long archiveSize, INotification[] notifications)
    {
        ArchiveId = archiveId;
        ArchiveType = archiveType;
        ArchiveSize = archiveSize;
        Notifications = notifications;
    }

    public static UploadArchiveSendloadOutput Factory(ArchiveValueObject archive)
        => new UploadArchiveSendloadOutput(
            archiveId: archive.GetArchiveId(),
            archiveType: archive.GetTypeArchive().ToString(),
            archiveSize: archive.GetArchiveSize(),
            notifications: archive.GetProcessResult().Notifications ?? []);
}
