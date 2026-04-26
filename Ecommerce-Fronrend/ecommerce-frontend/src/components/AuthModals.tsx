"use client";

import { motion, AnimatePresence } from "framer-motion";
import { useState, useEffect } from "react";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";
import { X } from "lucide-react";

interface AuthModalProps {
  isOpen: boolean;
  defaultTab: "login" | "register";
  onClose: () => void;
}

export default function AuthModal({ isOpen, defaultTab, onClose }: AuthModalProps) {
  const [activeTab, setActiveTab] = useState<"login" | "register">(defaultTab);

  useEffect(() => {
    setActiveTab(defaultTab);
  }, [defaultTab]);

  // close on escape key
  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", handleEsc);
    return () => window.removeEventListener("keydown", handleEsc);
  }, [onClose]);

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* backdrop — click to close */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.3 }}
            onClick={onClose}
            className="fixed inset-0 z-50 bg-black/30 backdrop-blur-sm"
          />

          {/* modal */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: -20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: -20 }}
            transition={{ duration: 0.35, ease: [0.16, 1, 0.3, 1] }}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 z-50 w-full max-w-md"
          >
            <div className="relative rounded-3xl border border-white/20 bg-white/10 backdrop-blur-2xl shadow-2xl overflow-hidden">
              
              {/* glass inner glow */}
              <div className="absolute inset-0 bg-gradient-to-br from-white/15 via-white/5 to-transparent pointer-events-none" />

              <div className="relative p-8">
                
                {/* close button */}
                <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }}
                  onClick={onClose}
                  className="absolute top-5 right-5 w-8 h-8 rounded-full bg-white/10 flex items-center justify-center text-white/60 hover:text-white hover:bg-white/20 transition-colors"
                >
                  <X size={14} />
                </motion.button>

                {/* tab toggle */}
                <div className="flex items-center bg-white/10 rounded-full p-1 mb-8">
                  {(["login", "register"] as const).map((tab) => (
                    <motion.button
                      key={tab}
                      onClick={() => setActiveTab(tab)}
                      className="relative flex-1 py-2 text-sm font-medium rounded-full transition-colors z-10"
                      style={{
                        color: activeTab === tab ? "#000" : "rgba(255,255,255,0.6)",
                      }}
                    >
                      {activeTab === tab && (
                        <motion.div
                          layoutId="activeTab"
                          className="absolute inset-0 bg-white rounded-full"
                          transition={{ type: "spring", bounce: 0.2, duration: 0.5 }}
                        />
                      )}
                      <span className="relative z-10 capitalize">
                        {tab === "login" ? "Sign in" : "Get started"}
                      </span>
                    </motion.button>
                  ))}
                </div>

                {/* form with animation */}
                <AnimatePresence mode="wait">
                  <motion.div
                    key={activeTab}
                    initial={{ opacity: 0, x: activeTab === "login" ? -20 : 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, x: activeTab === "login" ? 20 : -20 }}
                    transition={{ duration: 0.25, ease: "easeInOut" }}
                  >
                    {activeTab === "login" ? (
                      <LoginForm onSuccess={onClose} />
                    ) : (
                      <RegisterForm
                        onSuccess={() => setActiveTab("login")}
                      />
                    )}
                  </motion.div>
                </AnimatePresence>

                {/* toggle hint */}
                <p className="text-center text-sm text-white/40 mt-6">
                  {activeTab === "login" ? (
                    <>
                      Don&apos;t have an account?{" "}
                      <button
                        onClick={() => setActiveTab("register")}
                        className="text-white/70 hover:text-white underline transition-colors"
                      >
                        Get started
                      </button>
                    </>
                  ) : (
                    <>
                      Already have an account?{" "}
                      <button
                        onClick={() => setActiveTab("login")}
                        className="text-white/70 hover:text-white underline transition-colors"
                      >
                        Sign in
                      </button>
                    </>
                  )}
                </p>
              </div>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}