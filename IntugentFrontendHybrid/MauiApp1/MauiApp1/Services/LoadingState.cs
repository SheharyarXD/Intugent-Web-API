namespace MauiApp1.Services
{
    // Tracks in-flight API calls so the layout can show a loader while pages and charts wait for data.
    public class LoadingState
    {
        private int pending;

        public bool IsLoading => Volatile.Read(ref pending) > 0;

        public event Action? Changed;

        public void Begin()
        {
            if (Interlocked.Increment(ref pending) == 1) Changed?.Invoke();
        }

        public void End()
        {
            if (Interlocked.Decrement(ref pending) == 0) Changed?.Invoke();
        }
    }

    // Wraps every HttpClient request in LoadingState.Begin/End.
    public class LoadingHandler : DelegatingHandler
    {
        private readonly LoadingState loading;

        public LoadingHandler(LoadingState loading, HttpMessageHandler inner) : base(inner)
        {
            this.loading = loading;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            loading.Begin();
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                loading.End();
            }
        }
    }
}
