import { useMutation } from "@tanstack/react-query";
import { shopCategories } from "apps/seller-ui/src/utils/categories";
import axios from "axios";
import React from "react";
import { useForm } from "react-hook-form";

function CreateShop({
  sellerId,
  setActiveStep,
}: {
  sellerId: string;
  setActiveStep: (step: number) => void;
}) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const shopCreateMutation = useMutation({
    mutationFn: async (data: FormData) => {
      const response = await axios.post(
        `${process.env.NEXT_PUBLIC_SERVER_URI}/api/create-shop`,
        data
      );

      return response.data;
    },
    onSuccess: () => {
      setActiveStep(3);
    },
  });

  const onSubmit = async (data: any) => {
    const shopData = { ...data, sellerId };
    shopCreateMutation.mutate(shopData);
  };

  const countWords = (text: string) => text.trim().split(/\s+/).length;

  return (
    <div>
      <form onSubmit={handleSubmit(onSubmit)}>
        <h3 className="text-2xl font-semibold text-center mb-4">
          Setup new shop
        </h3>

        {/* Name */}
        <label className="block text-gray-700 mb-1">
          Name <span className=" text-red-500">*</span>
        </label>
        <input
          type="text"
          placeholder="shop name"
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-1"
          {...register("name", { required: "Name is required" })}
        />
        {errors.name && (
          <p className="text-red-500 text-sm">{String(errors.name.message)}</p>
        )}

        {/* Bio */}
        <label className="block text-gray-700 mb-1">
          Bio <span className=" text-red-500">*</span> (Max 100 words)
        </label>
        <input
          type="text"
          placeholder="shop bio"
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-1"
          {...register("bio", {
            required: "Bio is required",
            validate: (val) =>
              countWords(val) <= 100 || "Bio must be less than 100 words",
          })}
        />
        {errors.bio && (
          <p className="text-red-500 text-sm">{String(errors.bio.message)}</p>
        )}

        {/* Address */}
        <label className="block text-gray-700 mb-1">
          Address <span className=" text-red-500">*</span>
        </label>
        <input
          type="text"
          placeholder="shop location"
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-1"
          {...register("address", {
            required: "Shop Address is required",
          })}
        />
        {errors.address && (
          <p className="text-red-500 text-sm">
            {String(errors.address.message)}
          </p>
        )}

        {/* Opening hours */}
        <label className="block text-gray-700 mb-1">
          Opening Hours <span className=" text-red-500">*</span>
        </label>
        <input
          type="text"
          placeholder="e.g., Monday - Friday, 9am - 5pm"
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-1"
          {...register("opening_hours", {
            required: "Opening hours is required",
          })}
        />
        {errors.opening_hours && (
          <p className="text-red-500 text-sm">
            {String(errors.opening_hours.message)}
          </p>
        )}

        {/* Website */}
        <label className="block text-gray-700 mb-1">Website</label>
        <input
          type="text"
          placeholder="https://yourshop.com"
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-4"
          {...register("website", {
            pattern: {
              value: /^(https?:\/\/)?([\w-]+(\.[\w-]+)+)(\/[\w-]*)*\/?$/,
              message: "Enter a valid URL",
            },
          })}
        />
        {errors.website && (
          <p className="text-red-500 text-sm">
            {String(errors.website.message)}
          </p>
        )}

        {/* Categories */}
        <label className="block text-gray-700 mb-1">
          Category <span className=" text-red-500">*</span>
        </label>
        <select
          {...register("category", { required: "Category is required" })}
          className="w-full p-2 border border-gray-300 outline-0 rounded-[4px] mb-1">
            <option value="">Select a category</option>
            {shopCategories.map((category) => (
              <option key={category.value} value={category.value}>
                {category.label}
              </option>
            ))}
        </select>
        {errors.category && (
          <p className="text-red-500 text-sm">
            {String(errors.category.message)}
          </p>
        )}

        <button className="w-full bg-blue-500 p-2 mt-4 text-white rounded-lg text-lg">
          Create
        </button>
      </form>
    </div>
  );
}

export default CreateShop;
