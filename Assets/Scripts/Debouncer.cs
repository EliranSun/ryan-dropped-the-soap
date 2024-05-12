using System;
using System.Timers;

public class Debouncer {
    private readonly Timer _timer;

    public Debouncer(Action action, double intervalInMilliseconds) {
        _timer = new Timer(intervalInMilliseconds) {
            AutoReset = false // Ensure the timer only triggers once
        };
        _timer.Elapsed += (sender, args) => action();
    }

    public void Debounce() {
        // Reset the timer on every invocation to wait for the specified interval again
        _timer.Stop();
        _timer.Start();
    }
}