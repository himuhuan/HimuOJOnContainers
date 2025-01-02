import * as SignalR from "@microsoft/signalr";

export function createSubmitsHubConnection() {
	return new SignalR.HubConnectionBuilder()
		.withUrl("/submitshub", {
            skipNegotiation: true,
            transport: SignalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
		.build();
}
