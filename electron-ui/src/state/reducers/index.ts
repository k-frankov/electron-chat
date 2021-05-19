import { combineReducers } from "redux";
import userAuthStateReducer from "./userAuthStateReducer";

const reducers = combineReducers({
    userAuthState: userAuthStateReducer,
});

export default reducers;

export type State = ReturnType<typeof reducers>;