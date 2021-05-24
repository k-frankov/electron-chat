import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { store } from "../state";
import { gotChannels } from "../state/action-creators";

interface ChannelJoinedResponse {
  groupJoined: boolean,
  groupName: string,
}

class ChatHubConnection {
  hubConnection: HubConnection | null = null;
  createHubConnection = async (): Promise<boolean> => {
    let connectionEstablished = false;
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Critical)
      .withUrl("http://localhost:5000/chat", {
        accessTokenFactory: () => {
          if (store.getState().authenticatedUser !== null) {
            // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
            return store.getState().authenticatedUser!.jwtToken;
          } else {
            throw Error("User not authorized.");
          }
        },
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    await this.hubConnection
      .start()
      .then(() => {
        connectionEstablished = true;
      })
      .catch((error) =>
        console.log("Error establishing the connection: ", error),
      );

    this.hubConnection.on("GetChannels", (channels: string[]) => {
      store.dispatch(gotChannels(channels) as any);
    });

    this.hubConnection.on("ChannelJoined", (response: ChannelJoinedResponse) => {
      console.log(response);
    });

    return connectionEstablished;
  };

  joinChannel = (channel: string): void => {
    console.log(channel)
    this.hubConnection?.invoke("JoinChannel", channel)
      .catch((error) => console.log(error));
  };

  stopHubConnection = (): void => {
    this.hubConnection
      ?.stop()
      .catch((error) => console.log("Error stopping connection: ", error));
  };
}

export default ChatHubConnection;
