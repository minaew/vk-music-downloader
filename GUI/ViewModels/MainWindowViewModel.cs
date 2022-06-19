// #define TEST

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using ReactiveUI;

using MusicDownloader.Core;
using MusicDownloader.Logger;

namespace GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DateTime _start;
        private DateTime _end;

        public MainWindowViewModel()
        {
            End = DateTime.Now;
            Start = End - TimeSpan.FromDays(2);
        }

        public ObservableCollection<AudioFeed> Feeds { get; } = new ObservableCollection<AudioFeed>();

        public DateTime Start
        {
            get { return _start; }
            set { this.RaiseAndSetIfChanged(ref _start, value); }
        }

        public DateTime End
        {
            get { return _end; }
            set { this.RaiseAndSetIfChanged(ref _end, value); }
        }

        public async Task RefreshAsync()
        {
#if TEST

            Feeds.Clear();
            Feeds.Add(new AudioFeed(0, new Group { Photo = @"C:\Users\�������.000\AppData\Local\MusicDownloader\groups\14301.jpg" }, DateTimeOffset.Now, Enumerable.Empty<Audio>()));
            Feeds.Add(new AudioFeed(1, new Group { Photo = "ass1" }, DateTimeOffset.Now, Enumerable.Range(0, 20).Select(i => new Audio())));
            Feeds.Add(new AudioFeed(2, new Group { Photo = "ass1" }, DateTimeOffset.Now, Enumerable.Range(0, 20).Select(i => new Audio())));
            Feeds.Add(new AudioFeed(3, new Group { Photo = "ass1" }, DateTimeOffset.Now, Enumerable.Range(0, 20).Select(i => new Audio())));
            Feeds.Add(new AudioFeed(4, new Group { Photo = "ass1" }, DateTimeOffset.Now, Enumerable.Range(0, 20).Select(i => new Audio())));

            this.RaisePropertyChanged(nameof(Feeds));
#else
            using (var executor = new VkMethodExecutor(FileLogger.Default, new OfficialApp().UserAgent))
            {
                Feeds.Clear();
                await foreach (var feed in executor.GetFeedAsync(Start, End))
                {
                    Feeds.Add(feed);
                }
            }
#endif
        }

        public void Refresh()
        {
            RefreshAsync();
        }
    }
}
