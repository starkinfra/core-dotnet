﻿using System;
using System.Collections.Generic;
using StarkCore;


namespace StarkCore
{
    /// <summary>
    /// Project object
    /// <br/>
    /// The Project object is the main authentication entity for the SDK.
    /// All requests to the Stark Infra API must be authenticated via a project,
    /// which must have been previously created at the Stark Infra website
    /// [https://web.sandbox.starkbank.com] or [https://web.starkinfra.com]
    /// before you can use it in this SDK.Projects may be passed as a parameter on
    /// each request or may be defined as the default user at the start(See README).
    /// <br/>
    /// Properties:
    /// <list>
    ///     <item>ID [string]: unique id required to identify project. ex: "5656565656565656"</item>
    ///     <item>Environment [string]: environment where the project is being used. ex: "sandbox" or "production"</item>
    ///     <item>Name [string, default ""]: project name. ex: "MyProject"</item>
    ///     <item>AllowedIps [list of strings]: list containing the strings of the ips allowed to make requests on behalf of this project. ex: new List<string>{ "190.190.0.50" }</item>
    ///     <item>Pem [string]: private key in pem format. ex: "-----BEGIN PUBLIC KEY-----\nMFYwEAYHKoZIzj0CAQYFK4EEAAoDQgAEyTIHK6jYuik6ktM9FIF3yCEYzpLjO5X/\ntqDioGM+R2RyW0QEo+1DG8BrUf4UXHSvCjtQ0yLppygz23z0yPZYfw==\n-----END PUBLIC KEY-----"</item>
    /// </list>
    /// </summary>
    public class Project : User
    {
        public string Name { get; }
        public List<string> AllowedIps { get; }

        /// <summary>
        /// Project object
        /// <br/>
        /// The Project object is the main authentication entity for the SDK.
        /// All requests to the Stark Infra API must be authenticated via a project,
        /// which must have been previously created at the Stark Infra website
        /// [https://web.sandbox.starkbank.com] or [https://web.starkinfra.com]
        /// before you can use it in this SDK.Projects may be passed as a parameter on
        /// each request or may be defined as the default user at the start(See README).
        /// <br/>
        /// Parameters (required):
        /// <list>
        ///     <item>id [string]: unique id required to identify project. ex: "5656565656565656"</item>
        ///     <item>environment [string]: environment where the project is being used. ex: "sandbox" or "production"</item>
        ///     <item>privateKey [string]: PEM string of the private key linked to the project. ex: "-----BEGIN PUBLIC KEY-----\nMFYwEAYHKoZIzj0CAQYFK4EEAAoDQgAEyTIHK6jYuik6ktM9FIF3yCEYzpLjO5X/\ntqDioGM+R2RyW0QEo+1DG8BrUf4UXHSvCjtQ0yLppygz23z0yPZYfw==\n-----END PUBLIC KEY-----"</item>
        /// </list>
        /// <br/>
        /// Parameters (optional):
        /// <list>
        ///     <item>name [string, default ""]: project name. ex: "MyProject"</item>
        ///     <item>allowedIps [list of strings]: list containing the strings of the ips allowed to make requests on behalf of this project. ex: new List<string>{ "190.190.0.50" }</item>
        /// </list>
        /// </summary>
        public Project(string environment, string id, string privateKey, string name = "", List<string> allowedIps = null) : base(environment, id, privateKey)
        {
            Name = name;
            AllowedIps = allowedIps;
        }

        public override string AccessId()
        {
            return "project/" + ID;
        }
    }
}
