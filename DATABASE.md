## General Naming Conventions

1. Use PascalCase


## Schemas
Schemas should be used to organize related database objects such as tables. For example, all database objects related to the framework are in the [core] schema.

### Names
* Use names that represent a feature or a logic group of functionality.

## Tables


### Names
1. Table names should use the singular form. 
 1. A table that stores user records will be called User. 
 2. In the case of a join table, such as a table that stores the relationship between a role and claims, use the singular form: RoleClaim.
 
### Keys
Every table should have a primary key.

## Columns 

### Names
1. 

### Data Types
Columns should try to adhere to a standard set of data types based off of their content content.

|Primitive Type | Column Data Type | Additional Information
|:----|:----|:----|
|String | nvarchar | Enforce a maximum length when the possible set of values is well known|
| Bool | bit | Entityframework maps bit columns into a true/false boolean. Avoid using char(1) or other textual representations of bool values due to inconsistenancy in input as well as lack of meaning in other languages.|


### Operational Columns
Operational columns will be added to every table. The standard columns are:

| Column Name | Data Type | Nullable | Description
| ------------- |:-------------:| :-----:| :-----|
| CreateDate | DateTime | No | Row create date|
| CreateUser | nvarchar(50) | No | Create user |
| ModifyDate | DateTime | No | Row modify date | 
| ModifyUser | nvarchar(50) | No | Modify user |
| Deleted (Option A)    | bit | No    | Indicates if the record has been soft deleted|
| DeletedAt (Option B) | DateTime | Yes | Indicates the date the record was deleted|

## Stored Procedures

### Names
1. Do not prefix store procedures with sp_ 