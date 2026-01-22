import React from "react";

interface FormWrapperProps {
  title: string;
  onSubmit: (e: React.FormEvent) => void;
  children: React.ReactNode;
  titleClassName?: string;
}

export default function FormWrapper({ title, onSubmit, children, titleClassName = "" }: FormWrapperProps) {
  return (
    <div className="mb-8 rounded-md border border-gray-200 bg-white p-6 shadow-sm">
      <h2 className={`mb-4 text-lg font-semibold ${titleClassName}`}>{title}</h2>
      <form onSubmit={onSubmit} className="space-y-4 max-w-md">
        {children}
      </form>
    </div>
  );
}