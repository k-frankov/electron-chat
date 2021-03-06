import { combineReducers } from "redux";
import userAuthStateReducer from "./userAuthStateReducer";
import chatHubReducer from "./chatHubReducer";
import channelsReceived from "./channelsReceivedReducer";
import longOperation from "./longOperationReducer";
import channelJoined from "./channelJoinedReducer";

const reducers = combineReducers({
    authenticatedUser: userAuthStateReducer,
    chatHub: chatHubReducer,
    channels: channelsReceived,
    longOperation: longOperation,
    channelJoined: channelJoined,
});

export default reducers;

export type AuthenticatedUser = ReturnType<typeof reducers>;
export type ChatHub = ReturnType<typeof reducers>;
export type ChannelsReceived = ReturnType<typeof reducers>;
export type LongOperation = ReturnType<typeof reducers>;
export type ChannelJoined = ReturnType<typeof reducers>;