import socket
import csv

host, port = "127.0.0.1", 25001

# Create the socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Connect to the server
sock.connect((host, port))

# Read the CSV file and convert it into the expected format
csv_file = "path.csv"  # Path to your CSV file

csv_data = []
with open(csv_file, newline='') as f:
    reader = csv.reader(f)
    for row in reader:
        # Assuming each row has 3 values (x, y, z)
        # Format it as "(x,y,z)"
        formatted_row = f"({','.join(row)})"
        csv_data.append(formatted_row)

# Join all rows into a single CSV string
csv_string = ','.join(csv_data)

# Send the CSV data
sock.sendall(csv_string.encode("utf-8"))

# Receive the response from the server
response = sock.recv(1024).decode("utf-8")

print(response)

# Close the socket
sock.close()
