using System;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    // Singleton instance for easy access.
    public static Ticker Instance { get; private set; }

    // A list to store all the ticker tasks.
    private List<TickerTask> tasks = new List<TickerTask>();

    // Optional: if you want ticks at a fixed interval, you could use a timer.
    // For now, we'll call tasks every Update.
    public float tickInterval = 0f; // 0 means every frame.
    private float tickTimer = 0f;

    private void Awake()
    {
        // Set up the singleton instance.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist between scenes.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Register a new task to be called on each tick.
    /// The task should return false to keep running,
    /// and true when it is complete (so it can be unregistered).
    /// </summary>
    /// <param name="order">
    /// An integer that determines the order in which tasks run.
    /// Lower numbers are executed first.
    /// </param>
    /// <param name="task">The Func to call on each tick.</param>
    public void RegisterTask(int order, Func<bool> task)
    {
        tasks.Add(new TickerTask(order, task));
        // Ensure the list is sorted by the order value.
        tasks.Sort((a, b) => a.Order.CompareTo(b.Order));
    }

    /// <summary>
    /// Unregister a task so it no longer gets called.
    /// </summary>
    public void UnregisterTask(Func<bool> task)
    {
        tasks.RemoveAll(t => t.Task == task);
    }

    private void Update()
    {
        // If tickInterval is greater than zero, use a timer.
        if (tickInterval > 0f)
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                ExecuteTasks();
            }
        }
        else
        {
            // Otherwise, call every frame.
            ExecuteTasks();
        }
    }

    private void ExecuteTasks()
    {
        // Iterate backwards so that removal during iteration is safe.
        for (int i = tasks.Count - 1; i >= 0; i--)
        {
            Debug.Log("Executing task: " + tasks[i].Order);
            // If the task returns true, it signals it is complete.
            bool taskCompleted = tasks[i].Task?.Invoke() ?? true;
            if (taskCompleted)
            {
                tasks.RemoveAt(i);
            }
        }
    }
}

/// <summary>
/// Helper class representing a task that runs on a tick.
/// </summary>
public class TickerTask
{
    public int Order { get; private set; }
    public Func<bool> Task { get; private set; }

    public TickerTask(int order, Func<bool> task)
    {
        Order = order;
        Task = task;
    }
}
