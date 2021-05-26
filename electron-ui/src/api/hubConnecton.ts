import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { store } from "../state";
import {
  gotChannels,
  longOperationSwitch,
  channelJoined,
} from "../state/action-creators";
import { ChatMessage } from "../state/models/chatMessage";

interface ChannelJoinedResponse {
  groupJoined: boolean;
  groupName: string;
}

interface ChannelUsersCallback {
  (users: string[]): void;
}

interface ChannelOldMessagesCallback {
  (messages: ChatMessage[]): void;
}

interface ChannelNewMessageCallback {
  (newMessage: ChatMessage): void;
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

    this.hubConnection.on(
      "ChannelJoined",
      (response: ChannelJoinedResponse) => {
        store.dispatch(longOperationSwitch() as any);
        if (response.groupJoined) {
          store.dispatch(channelJoined(response.groupName) as any);
        }
      },
    );

    this.hubConnection.on("GetUsersInChannel", (resp: string[]) => {
      if (resp !== undefined && resp !== null) {
        if (this.channelUsersCallback !== null) {
          this.channelUsersCallback(resp);
        }
      }
    });

    this.hubConnection.on("GetMessagesInChannel", (resp: ChatMessage[]) => {
      if (resp !== undefined && resp !== null) {
        if (this.channelOldMessagesCallback !== null) {
          this.channelOldMessagesCallback(resp);
        }
      }
    });

    this.hubConnection.on("GetMessageInChannel", (resp: ChatMessage) => {
      if (resp !== undefined && resp !== null) {
        if (this.channelNewMessageCallback !== null) {
          this.channelNewMessageCallback(resp);
        }
      }
    });

    return connectionEstablished;
  };

  channelUsersCallback: ChannelUsersCallback;
  channelOldMessagesCallback: ChannelOldMessagesCallback;
  channelNewMessageCallback: ChannelNewMessageCallback;

  joinChannel = (channel: string): void => {
    store.dispatch(longOperationSwitch() as any);
    this.hubConnection
      ?.invoke("JoinChannel", channel)
      .catch((error) => console.log(error));
  };

  sendMessageToChannel = (channel: string, message: string): void => {
    this.hubConnection
      ?.invoke("SendMessageToChannel", channel, message)
      .catch((error) => console.log(error));
  };

  stopHubConnection = (): void => {
    this.hubConnection
      ?.stop()
      .catch((error) => console.log("Error stopping connection: ", error));
  };
}

export default ChatHubConnection;
