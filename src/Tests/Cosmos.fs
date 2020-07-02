﻿module Cosmos

open Expecto
open Farmer
open Farmer.Builders
open Farmer.Storage
open Microsoft.Azure.Management.Storage
open Microsoft.Azure.Management.Storage.Models
open Microsoft.Rest
open System

let tests = testList "Cosmos" [
    test "Cosmos container should ignore duplicate unique keys" {

        let container =
            cosmosContainer {
                name "people"
                partition_key [ "/id" ] CosmosDb.Hash
                add_unique_key [ "/FirstName" ]
                add_unique_key [ "/LastName" ]
                add_unique_key [ "/LastName" ]
            }

        Expect.equal container.UniqueKeys.Count 2 "There should be 2 unique keys."
        Expect.contains container.UniqueKeys ["/FirstName"] "UniqueKeys should contain /FirstName"
        Expect.contains container.UniqueKeys ["/LastName"] "UniqueKeys should contain /LastName"
    }
    test "DB properties are correctly evaluated" {
        let db = cosmosDb { name "test" }
        Expect.equal "[reference(concat('Microsoft.DocumentDb/databaseAccounts/', 'test-server')).documentEndpoint]" (db.Endpoint.Eval()) "Endpoint is incorrect"
        Expect.equal "[listKeys(resourceId('Microsoft.DocumentDB/databaseAccounts', 'test-server'), providers('Microsoft.DocumentDB','databaseAccounts').apiVersions[0]).primaryMasterKey]" (db.PrimaryKey.Eval()) "Primary Key is incorrect"
    }
]