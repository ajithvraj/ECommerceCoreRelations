"use client";

import { useState } from "react";
import Header from "@/components/Header";
import AuthModal from "@/components/AuthModals";

export default function UserLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const [modalOpen, setModalOpen] = useState(false);
  const [activeTab, setActiveTab] = useState<"login" | "register">("login");

  return (
    <>
      <Header
        onLoginClick={() => {
          setActiveTab("login");
          setModalOpen(true);
        }}
        onRegisterClick={() => {
          setActiveTab("register");
          setModalOpen(true);
        }}
      />
      <AuthModal
        isOpen={modalOpen}
        defaultTab={activeTab}
        onClose={() => setModalOpen(false)}
      />
      {children}
    </>
  );
}