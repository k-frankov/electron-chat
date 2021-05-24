import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { store } from "../state";
import { gotChannels, longOperationSwitch, } from "../state/action-creators";

interface ChannelJoinedResponse {
  groupJoined: boolean,
  groupName: string,
}

class ChatHubConnection {
  hubConnection: HubConnection | null = null;
  createHubConnection = async (): Promise<boolean> => {
    let connectionEstablished = false;
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Error)
      .withUrl("http://localhost:5100/chat", {
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
      .build();

    await this.hubConnection
      .start()
      .then(() => {
        connectionEstablished = true;
      })
      .catch((error) => {
        console.log("Error establishing the connection: ", error);
      });

    this.hubConnection.on("GetChannels", (channels: string[]) => {
      store.dispatch(gotChannels(channels) as any);
    });

    this.hubConnection.on("ChannelJoined", (response: ChannelJoinedResponse) => {
      store.dispatch(longOperationSwitch() as any);
      console.log(response);
    });

    return connectionEstablished;
  };

  joinChannel = (channel: string): void => {
    store.dispatch(longOperationSwitch() as any);
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
