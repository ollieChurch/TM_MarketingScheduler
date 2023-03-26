# Marketing Scheduler

This is a simple .NET 6.0 web application for scheduling marketing emails for customers. Customers can be added to the database and scheduled to receive emails at different frequencies.

## Set Up

1. Clone the repository from GitHub.
2. Make sure you have installed .NET 6.0 SDK or later.
3. Create a **`.env`** file at the root of the project folder. and add the following line to the **`.env`** file, replacing **`<connection string>`** with the actual MongoDB connection string you have been provided:
    
    ```
    DB_CONN_STR=<connection string>
    ```
    
4. Open a terminal/command prompt and navigate to the project folder. Run the following command to restore the project dependencies:
    
    ```
    dotnet restore
    ```
    
5. Run the following command to build the project:
    
    ```
    dotnet build
    ```
    
6. Finally, run the following command to start the project:
    
    ```
    dotnet run
    ```
    

This should start the project and make it accessible via a web browser.

## User Guide

Open the Swagger UI by navigating to the URL where the project is running, followed by **`/swagger`**.

Endpoints:

1. GET **`/MarketingScheduler/customers`**: Retrieves a list of all customers.
    - Expected response: A list of Customer objects.
    
2. POST **`/MarketingScheduler/addCustomers`**: Adds a list of new customer to the database.
    
    The input here is a JSON list of new customers. The system will assign an Id to  each new customer so there is no need to include this in the input.
    
    As part of each new customer you will need to define the frequency as an enum. These are:
    
    ```jsx
    Never = 0
    Daily = 1
    Weekly = 2
    Monthly = 3
    ```
    
    When choosing *Weekly* or *Monthly* frequency you will also need to include a frequencyDetails array to identify when in the week or month the customer wishes to receive marketing.
    
    For ******Weekly****** the array will include enums from 0 (Sunday) to 6 (Saturday).
    
    For Monthly the array will include integers representative of date from 1 (1st) to 28 (28th)
    
    Example input:
    
    ```
    [
    	{
    	  "name": "Jane Smith",
    	  "frequency": 1
    	},
    	{
    	  "name": "Jemima Smith",
    	  "frequency": 2,
    	  "frequencyDetails": [0,6]
    	},
    	{
    	  "name": "Julie Smith",
    	  "frequency": 3,
    	  "frequencyDetails": [10,24]
    	}
    ]
    ```
    
    In the above example:
    
    - Jane is expecting to receive marketing daily, so no frequencyDetails are required
    - Jemima is expecting to receive marketing every Sunday and Saturday
    - Julie is expecting to receive marketing on the 10th and 24th of each month
    
    - Expected response: report for the next 90 days detailing who is receiving marketing on each day.
    
3. GET `/MarketingScheduler/{numberOfDays}/report`: Returns a report for the specified number of days detailing who will be receiving marketing on each day
    
    - Expected response: report for the specified number of days detailing who is receiving marketing on each day.
    
4. PATCH `/MarketingScheduler/{customerId}/updatePreference`: Updates an existing customer’s marketing preference in the database.
    
    You will need the Id for the customer you wish to update and you can select from the dropdown the frequency enum, referencing the same frequencies listed above.
    
    If you choose *Weekly* or *Monthly* you will also need to include a frequencyDetails array. Remember for ******Weekly****** the array will include enums from 0 (Sunday) to 6 (Saturday) and for Monthly the array will include integers representative of date from 1 (1st) to 28 (28th).
    
    Expected response: The updated Customer object.

5. DELETE `/MarketingScheduler/{customerId}/delete`: Deletes an existing customer from the database using the provided customer Id.

## What Next?

Given this is a product which would handle customer's data, authentication of the user would be the next consideration for this project. Depending of the scope of it's usage, this could be via API key or, ideally, there would be a frontend calling these endpoints and so it would be part of a larger authentication system. Maybe in this case authentication would be by JWT. 

I would have liked to include a search endpoint which could provide back a list of customers for specified search parameters, such as daily frequency, weekly frequency on a Sunday or by name.
It would also be relatively easy to expand the customer model to include contact details for the customer (e.g. email) and then maybe some basic validations, such as to prevent the same email being used twice.
