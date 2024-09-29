
# CalcCommute

CalcCommute is a C# console application that calculates the commute time and distance between two addresses using the Google Maps API. It prompts the user for an origin and destination, then fetches and displays the travel time and distance based on the data retrieved from Google Maps.

## Features

- Fetches real-time commute data from Google Maps API.
- Calculates total distance in miles.
- Calculates travel duration in minutes and hours.
- Reads Google Maps API key from a file (`Maps-API.txt`).

## Requirements

- .NET Core SDK (3.1 or later)
- Google Maps API key (saved in a `Maps-API.txt` file in the project directory)
- Newtonsoft.Json NuGet package for JSON handling

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/anadeem717/CalcCommute.git
   ```

2. Navigate to the project directory:

   ```bash
   cd CalcCommute
   ```

3. Install the necessary dependencies (e.g., `Newtonsoft.Json`):

   ```bash
   dotnet add package Newtonsoft.Json
   ```

4. Create a `Maps-API.txt` file in the project root directory and paste your Google Maps API key into it.

5. Build the project:

   ```bash
   dotnet build
   ```

6. Run the application:

   ```bash
   dotnet run
   ```

## Usage

1. Run the program. It will prompt you to input:
   - **Origin**: The starting address or city/state.
   - **Destination**: The destination address or city/state.

2. The application will fetch the commute details from Google Maps and display the following:
   - **Total Duration**: Commute time in minutes and hours.
   - **Total Distance**: Commute distance in miles.

## Example

```bash
Enter origin as {address} or {city, state}: Chicago, IL
Enter destination address as {address} or {city, state}: Evanston, IL
Total Duration: 34.15 min / 0.57 hrs
Total Distance: 13.58 miles
```

## API Key Setup

1. Obtain a Google Maps API key from the [Google Cloud Console](https://console.cloud.google.com/).
2. Save the API key in a file named `Maps-API.txt` located in the project root directory.
3. The application will read the API key from this file during execution.
