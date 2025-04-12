
# TestAccountCreator

DESCRIPTION: 
- Create one or more test accounts.

> NOTES: Code in initial repo commit is from 2011. 

## Requirements:

Operating System Requirements:
- Windows Server 2003 or higher (32-bit)
- Windows Server 2008 or higher (32-bit)

Additional software requirements:
Microsoft .NET Framework v3.5

Additional requirements:
Administrative access is required to perform operations by TestAccountCreator


## Operation and Configuration:

Command-line parameters:
- run (Required parameter)
- name (specify the name prefix for the test account)
- num (specify the number of test accounts to create)
- pass (specifiy the password for test accounts)

Examples:
TestAccountCreator -run -name:TestUser -num:100 -pass:TestPass00
