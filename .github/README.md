<div id="top"></div>

<!-- Readme template from https://github.com/othneildrew/Best-README-Template -->

<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Apache License 2.0 License][license-shield]][license-url]



<div align="center">

<h1 align="center">sentinel</h3>

  <p align="center">
    Remote Monitoring and Management (RMM) Software.
    <br />
    <a href="https://JelleBuning/sentinel/not_found">Demo</a>
    ·
    <a href="https://github.com/JelleBuning/sentinel/wiki">Explore the docs</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#features">Features</a></li>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

Sentinel is a powerful, scalable, and secure Remote Monitoring and Management (RMM) platform designed to help IT professionals and Managed Service Providers (MSPs) efficiently manage, monitor, and secure client devices and networks from a single dashboard.

With Sentinel, you'll gain full visibility and control over endpoints, proactive alerts, and automation tools that simplify maintenance tasks and increase operational efficiency.


### Features
- **Comprehensive Monitoring:** Real-time performance metrics and system health indicators for servers and workstations.
- **Automated Maintenance:** Schedule automated updates, patches, and maintenance tasks.
- **Security Management:** Integrated threat detection and response to protect endpoints from vulnerabilities.
- **Customizable Alerts:** Stay informed with tailored alerting systems for critical events.
- **Remote Access:** Secure, fast, and reliable remote desktop access.
- **Reporting & Analytics:** Generate detailed reports on system performance, compliance, and SLA adherence.
- **User Management:** Granular role-based access control (RBAC).
- **Integration-Friendly:** Connect with third-party tools such as ticketing systems, antivirus software, and more.


### Built With

* [.NET](https://dotnet.microsoft.com/en-us/)
* [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)


<!-- GETTING STARTED -->
## Getting Started
Setting up this solution on your local machine is straightforward and will enable you to fully utilize its capabilities. This guide will walk you through the necessary steps to get everything running smoothly.

Before beginning, ensure that your development environment is properly configured. Having the required software and dependencies installed will prevent common issues and streamline the process.

### Prerequisites

### Installation
This installation method utilizes Docker Compose for a streamlined setup. Ensure you have Docker and Docker Compose installed on your system.

1.  **Create a `docker-compose.yml` file:**
    Create a new file named `docker-compose.yml` in a directory of your choice. Copy and paste the following content into it:

    ```yaml
    version: '3.4'
    name: sentinel
    services:
      sentinel.api:
        container_name: "sentinel.api"
        image: ghcr.io/jellebuning/sentinel.api
        ports:
          - "7000:8080"
        environment:
          ASPNETCORE_ENVIRONMENT: "Production" # "Development" | "Production" | "Staging"
          ConnectionStrings__Database: "CONNECTIONSTRING_HERE"

    ```

2.  **Run Docker Compose:**
    In the same directory as your `docker-compose.yml` file, execute the following command:

    ```bash
    docker-compose up -d
    ```

    This command will download the necessary images, create the containers, and start them in detached mode.


4.  **Access the API:**
    The API will be available at `http://localhost:7000`.

**Important Notes:**

* Replace `"CONNECTIONSTRING_HERE"` with a your database connectionstring.


<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


<!-- LICENSE -->
## License

Distributed under the Apache 2.0 License. See `LICENSE` for more information.


<p align="right">(<a href="#top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/JelleBuning/sentinel.svg?style=for-the-badge
[contributors-url]: https://github.com/JelleBuning/sentinel/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/JelleBuning/sentinel.svg?style=for-the-badge
[forks-url]: https://github.com/JelleBuning/sentinel/network/members
[stars-shield]: https://img.shields.io/github/stars/JelleBuning/sentinel.svg?style=for-the-badge
[stars-url]: https://github.com/JelleBuning/sentinel/stargazers
[issues-shield]: https://img.shields.io/github/issues/JelleBuning/sentinel.svg?style=for-the-badge
[issues-url]: https://github.com/JelleBuning/sentinel/issues
[license-shield]: https://img.shields.io/github/license/JelleBuning/sentinel.svg?style=for-the-badge
[license-url]: https://github.com/JelleBuning/sentinel/blob/master/LICENSE
