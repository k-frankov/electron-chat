import { ActionType } from "../action-types";
import { LongOperation } from "../actions";

const initialState = false;

const reducer = (state = initialState, action: LongOperation): boolean => {
    switch (action.type) {
        case ActionType.LONGOPERATION:
            return !state;
        default: return false;
    }
}

export default reducer;