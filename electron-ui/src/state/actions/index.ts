import ChatHubConnection from "../../api/hubConnecton";
import { ActionType } from "../action-types";
import { ChatUser } from "../models/chatUser";

interface UserLoggedInActoin {
  type: ActionType.USER_LOGGED_IN;
  payload: ChatUser;
}

interface UserLoggedOutAction {
  type: ActionType.USER_LOGGED_OUT;
}

interface ChatHubConnectionSetAction {
  type: ActionType.CHAT_HUB_CREATED;
  payload: ChatHubConnection;
}

interface ChatHubConnectionRemovedAction {
  type: ActionType.CHAT_HUB_REMOVED;
}

interface ChannelsReceivedAction {
  type: ActionType.CHANNELS_RECEIVED;
  payload: string[];
}

interface LongOperationAction {
  type: ActionType.LONGOPERATION;
}

interface ChannelJoinedAction {
  type: ActionType.CHANNEL_JOINED;
  payload: string;
}

export type ChannelJoined = ChannelJoinedAction;
export type LongOperation = LongOperationAction;
export type ChannelsReceived = ChannelsReceivedAction;
export type UserAuthAction = UserLoggedInActoin | UserLoggedOutAction;
export type ChatHubConnectionStateAction =
  | ChatHubConnectionSetAction
  | ChatHubConnectionRemovedAction;
