import { ChatHubConnectionStateAction } from "../actions/index"
import { ActionType } from "../action-types/index"
import ChatHubConnection from "../../api/hubConnecton";

const initialState = null;

const reducer = (state = initialState, action: ChatHubConnectionStateAction): ChatHubConnection | null => {
    switch (action.type) {
        case ActionType.CHAT_HUB_CREATED:
            return action.payload;
        case ActionType.CHAT_HUB_REMOVED:
            return null;
        default: return state;
    }
}

export default reducer;