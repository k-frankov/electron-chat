import { combineReducers } from "redux";
import userAuthStateReducer from "./userAuthStateReducer";
import chatHubReducer from "./chatHubReducer";
import channelsReceived from "./channelsReceivedReducer";

const reducers = combineReducers({
    authenticatedUser: userAuthStateReducer,
    chatHub: chatHubReducer,
    channels: channelsReceived,
});

export default reducers;

export type AuthenticatedUser = ReturnType<typeof reducers>;
export type ChatHub = ReturnType<typeof reducers>;
export type ChannelsReceived = ReturnType<typeof reducers>;