using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class MyListener : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;
    Queue<Vector3> targetPositions = new Queue<Vector3>(); // Queue to store positions sequentially
    float moveSpeed = 2f; // Speed at which the object moves between positions
    Vector3 currentTargetPosition = Vector3.zero;
    bool isMoving = false;
    bool isFirstPosition = true; // Flag to track if we're on the first position

    void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();

        // Start listening
        running = true;
        while (running)
        {
            Connection();
        }
        server.Stop();
    }

    void Connection()
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        // Make sure we're not getting an empty string
        if (!string.IsNullOrEmpty(dataReceived))
        {
            // Parse the received CSV data into a list of Vector3 positions
            List<Vector3> positions = ParseCSVData(dataReceived);

            // Add the positions to the queue
            foreach (var pos in positions)
            {
                targetPositions.Enqueue(pos);
            }
        }
    }

    // Parse the CSV data into a list of Vector3 positions
    List<Vector3> ParseCSVData(string dataString)
    {
        List<Vector3> positions = new List<Vector3>();

        // Remove any unnecessary spaces and parentheses
        dataString = dataString.Trim();
        if (dataString.StartsWith("(") && dataString.EndsWith(")"))
        {
            dataString = dataString.Substring(1, dataString.Length - 2);
        }

        // Split the data by commas (assumed CSV format)
        string[] stringArray = dataString.Split(new string[] { "),(" }, System.StringSplitOptions.None);

        foreach (var item in stringArray)
        {
            string trimmedItem = item.Trim('(', ')');
            string[] coordinates = trimmedItem.Split(',');

            if (coordinates.Length == 3)
            {
                float x = float.Parse(coordinates[0]);
                float y = float.Parse(coordinates[1]);
                float z = float.Parse(coordinates[2]);

                positions.Add(new Vector3(x, y, z));
            }
        }

        return positions;
    }

    void Update()
    {
        // Move the object sequentially through positions if there are any left in the queue
        if (targetPositions.Count > 0)
        {
            if (isFirstPosition)
            {
                // Teleport to the first position (no smooth movement)
                currentTargetPosition = targetPositions.Dequeue();
                transform.position = currentTargetPosition;  // Teleport to the first position
                isFirstPosition = false; // After teleport, no longer the first position
                return;
            }

            if (!isMoving)
            {
                // Start moving towards the next target position
                currentTargetPosition = targetPositions.Dequeue();
                isMoving = true;
            }

            // Move towards the current target position smoothly
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, step);

            // Check if we have reached the target position
            if (transform.position == currentTargetPosition)
            {
                isMoving = false; // Stop moving to the next target
            }
        }
    }
}
