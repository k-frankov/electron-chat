import { ActionType } from "../action-types";
import { ChannelJoined } from "../actions";

const initialState = null;

const reducer = (state = initialState, action: ChannelJoined): string | null => {
    switch (action.type) {
        case ActionType.CHANNEL_JOINED:
            return action.payload;
        default: return state;
    }
}

export default reducer;