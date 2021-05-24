import { ActionType } from "../action-types";
import { ChannelsReceived } from "../actions";

const initialState: string[] = [];

const reducer = (state = initialState, action: ChannelsReceived): string[] => {
    switch (action.type) {
        case ActionType.CHANNELS_RECEIVED:
            return action.payload;
        default: return state;
    }
}

export default reducer;