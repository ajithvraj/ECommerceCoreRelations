"use client";

import { motion } from "framer-motion";
import useAuthStore from "@/store/authStore";
import api from "@/lib/axios";
import { toast } from "sonner";

interface HeaderProps {
  onLoginClick: () => void;
  onRegisterClick: () => void;
}

export default function Header({ onLoginClick, onRegisterClick }: HeaderProps) {
  const { user, isAuthenticated, clearAuth } = useAuthStore();

  const handleLogout = async () => {
    try {
      await api.post("/customer/revoke-token");
    } catch {
      // ignore error
    } finally {
      clearAuth();
      toast.success("Logged out successfully");
    }
  };

  return (
    <motion.header
      initial={{ y: -100, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.6, ease: "easeOut" }}
      className="fixed top-0 left-0 right-0 z-40 h-14"
    >
      <div className="absolute inset-0 bg-black/70 backdrop-blur-md border-b border-white/10" />
      <div className="relative max-w-7xl mx-auto px-6 h-full flex items-center justify-between">
        
        {/* Logo */}
        <motion.span
          whileHover={{ scale: 1.02 }}
          className="text-white font-semibold text-lg tracking-tight cursor-pointer"
        >
          ECommerceCore
        </motion.span>

        {/* Nav links */}
        <nav className="hidden md:flex items-center gap-8">
          {["Products", "Categories", "Deals"].map((item) => (
            <motion.a
              key={item}
              href="#"
              whileHover={{ color: "#ffffff" }}
              className="text-sm text-zinc-400 hover:text-white transition-colors"
            >
              {item}
            </motion.a>
          ))}
        </nav>

        {/* Auth buttons */}
        <div className="flex items-center gap-3">
          {isAuthenticated && user ? (
            <div className="flex items-center gap-4">
              <span className="text-sm text-zinc-300">
                Hi, {user.name.split(" ")[0]}
              </span>
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                onClick={handleLogout}
                className="text-sm text-zinc-400 hover:text-white transition-colors"
              >
                Logout
              </motion.button>
            </div>
          ) : (
            <>
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                onClick={onLoginClick}
                className="text-sm text-zinc-300 hover:text-white transition-colors px-4 py-1.5"
              >
                Sign in
              </motion.button>
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                onClick={onRegisterClick}
                className="text-sm bg-white text-black px-4 py-1.5 rounded-full hover:bg-zinc-200 transition-colors"
              >
                Get started
              </motion.button>
            </>
          )}
        </div>
      </div>
    </motion.header>
  );
}