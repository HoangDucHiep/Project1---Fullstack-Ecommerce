"use client";

import useSeller from "apps/seller-ui/src/hooks/useSeller";
import useSidebar from "apps/seller-ui/src/hooks/useSidebar";
import { usePathname } from "next/navigation";
import React, { useEffect } from "react";
import Box from "../box";
import Link from "next/link";
import Logo from "apps/seller-ui/src/assets/svgs/logo";
import { Sidebar } from "./sidebar.styles";
import SidebarItem from "./sidebar.item";
import {
  BellPlus,
  BellRing,
  BellRingIcon,
  CalendarPlus,
  Calendars,
  Headset,
  Home,
  ListOrdered,
  LogOut,
  Mail,
  PackageSearch,
  Settings,
  SquarePlus,
  TicketPercent,
} from "lucide-react";
import SidebarMenu from "./sidebar.menu";
import Payment from "apps/seller-ui/src/assets/svgs/payment";

const SidebarBarWrapper = () => {
  const { activeSidebar, setActiveSidebar } = useSidebar();
  const pathName = usePathname();
  const { seller } = useSeller();

  useEffect(() => {
    setActiveSidebar(pathName);
  }, [pathName, setActiveSidebar]);

  const getIconColor = (route: string) =>
    activeSidebar === route ? "#0085ff" : "#969696";

  return (
    <Box
      css={{
        height: "100vh",
        zIndex: 202,
        position: "sticky",
        padding: "8px",
        top: "0",
        overflowY: "scroll",
        scrollbarWidth: "none",
      }}
      className=" sidebar-wrapper"
    >
      <Sidebar.Header>
        <Box>
          <Link
            href="/"
            className="flex items-center justify-center text-center gap-5"
          >
            <Logo />
            <Box className="flex flex-col justify-center">
              <h3 className="text-lg font-semibold text-[#ecedee] leading-tight">
                {seller?.shop?.name}
              </h3>

{/*               <h5 className="font-medium text-xs text-[#ecedeecf] whitespace-nowrap overflow-hidden text-ellipsis max-w-[170px] leading-tight mt-0.5">
                {seller?.shop?.address}
              </h5> */}
            </Box>
          </Link>
        </Box>
      </Sidebar.Header>

      <div className="block my-3 h-full">
        <Sidebar.Body className="body sidebar">
          <SidebarItem
            title="Dashboard"
            icon={<Home size={25} color={getIconColor("/dashboard")} />}
            isActive={activeSidebar === "/dashboard"}
            href={"/dashboard"}
          />

          <div className="mt-2 block">
            <SidebarMenu title="Main Menu">
              <SidebarItem
                isActive={activeSidebar === "/dashboard/orders"}
                title="Orders"
                href={"/dashboard/orders"}
                icon={
                  <ListOrdered
                    size={25}
                    color={getIconColor("/dashboard/orders")}
                  />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/dashboard/payments"}
                title="Payments"
                href={"/dashboard/payments"}
                icon={<Payment fill={getIconColor("/dashboard/payments")} />}
              />
            </SidebarMenu>

            <SidebarMenu title="Products">
              <SidebarItem
                isActive={activeSidebar === "/dashboard/create-product"}
                title="Create Product"
                href={"/dashboard/create-product"}
                icon={
                  <SquarePlus
                    size={25}
                    color={getIconColor("/dashboard/create-product")}
                  />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/dashboard/all-products"}
                title="All Products"
                href={"/dashboard/all-products"}
                icon={
                  <PackageSearch
                    size={25}
                    color={getIconColor("/dashboard/all-products")}
                  />
                }
              />
            </SidebarMenu>

            <SidebarMenu title="Events">
              <SidebarItem
                isActive={activeSidebar === "/dashboard/create-event"}
                title="Create Event"
                href={"/dashboard/create-event"}
                icon={
                  <CalendarPlus
                    size={24}
                    color={getIconColor("/dashboard/create-event")}
                  />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/dashboard/all-events"}
                title="All Events"
                href={"/dashboard/all-events"}
                icon={
                  <Calendars
                    size={25}
                    color={getIconColor("/dashboard/all-events")}
                  />
                }
              />
            </SidebarMenu>

            <SidebarMenu title="Controllers">
              <SidebarItem
                isActive={activeSidebar === "/dashboard/inbox"}
                title="Inbox"
                href={"/dashboard/inbox"}
                icon={
                  <Mail size={24} color={getIconColor("/dashboard/inbox")} />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/dashboard/settings"}
                title="Settings"
                href={"/dashboard/settings"}
                icon={
                  <Settings
                    size={25}
                    color={getIconColor("/dashboard/settings")}
                  />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/dashboard/notifications"}
                title="Notifications"
                href={"/dashboard/notifications"}
                icon={
                  <BellRingIcon
                    size={25}
                    color={getIconColor("/dashboard/notifications")}
                  />
                }
              />
            </SidebarMenu>

            <SidebarMenu title="Extras">
              <SidebarItem
                isActive={activeSidebar === "/dashboard/discount-codes"}
                title="Discount Codes"
                href={"/dashboard/discount-codes"}
                icon={
                  <TicketPercent
                    size={24}
                    color={getIconColor("/dashboard/discount-codes")}
                  />
                }
              />

              <SidebarItem
                isActive={activeSidebar === "/logout"}
                title="Logout"
                href={"logout"}
                icon={<LogOut size={25} color={getIconColor("logout")} />}
              />
            </SidebarMenu>
          </div>
        </Sidebar.Body>
      </div>
    </Box>
  );
};

export default SidebarBarWrapper;
