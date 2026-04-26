import { create } from "zustand";
import { AuthResponse } from "@/types/auth";

interface AuthState {
  user: AuthResponse | null;
  isAuthenticated: boolean;
  setAuth: (user: AuthResponse) => void;
  clearAuth: () => void;
}

const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,

  setAuth: (user) => {
    localStorage.setItem("accessToken", user.token);
    localStorage.setItem("refreshToken", user.refreshToken);
    set({ user, isAuthenticated: true });
  },

  clearAuth: () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    set({ user: null, isAuthenticated: false });
  },
}));

export default useAuthStore;