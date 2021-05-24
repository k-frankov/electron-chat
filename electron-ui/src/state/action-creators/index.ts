import { ActionType } from "../action-types";
import { Dispatch } from "redux";
import { UserAuthAction, ChatHubConnectionStateAction, ChannelsReceived } from "../actions";
import { ChatUser } from "../models/chatUser";
import ChatHubConnection from "../../api/hubConnecton";

export const logIn = (payload: ChatUser) => {
  return (dispatch: Dispatch<UserAuthAction>): void => {
    dispatch({
      type: ActionType.USER_LOGGED_IN,
      payload: payload,
    });
  };
};

export const logOut = () => {
  return (dispatch: Dispatch<UserAuthAction>): void => {
    dispatch({
      type: ActionType.USER_LOGGED_OUT,
    });
  };
};

export const setHubChat = (payload: ChatHubConnection) => {
  return (dispatch: Dispatch<ChatHubConnectionStateAction>): void => {
    dispatch({
      type: ActionType.CHAT_HUB_CREATED,
      payload: payload,
    });
  };
};

export const removeHubChat = () => {
  return (dispatch: Dispatch<ChatHubConnectionStateAction>): void => {
    dispatch({
      type: ActionType.CHAT_HUB_REMOVED,
    });
  };
};

export const gotChannels = (payload: string[]) => {
  return (dispatch: Dispatch<ChannelsReceived>): void => {
    dispatch({
      type: ActionType.CHANNELS_RECEIVED,
      payload: payload,
    });
  }
} 
