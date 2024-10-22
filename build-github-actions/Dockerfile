FROM catthehacker/ubuntu:act-latest

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

RUN sudo apt-get update && \
sudo apt-get install -y dotnet-sdk-8.0

RUN sudo apt-get update && \
sudo apt-get install -y aspnetcore-runtime-8.0