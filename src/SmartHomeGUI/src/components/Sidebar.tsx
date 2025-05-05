import {Brightness4, Brightness7, Menu, Settings} from "@mui/icons-material"
import {Box, Divider, Drawer, List, ListItemButton, ListItemIcon, ListItemText, Switch} from "@mui/material"
import {useTheme} from "@mui/material/styles"
import {useCallback, useEffect, useState} from "react"
import {Link, useLocation} from "react-router-dom"
import {API_BASE_URL} from "../Config"
import {MeasurementLink} from "./Types"

interface SidebarProps
{
	menu: MeasurementLink[]
	isDarkTheme: boolean
	toggleTheme: () => void
}

const Sidebar = ({menu, isDarkTheme, toggleTheme}: SidebarProps) =>
{
	const theme = useTheme()
	const [icons, setIcons] = useState<{[key: string]: string}>({})
	const [collapsed, setCollapsed] = useState(false)
	const location = useLocation()

	const currentPathDecoded = decodeURIComponent(location.pathname)

	const fetchIcons = useCallback(async () =>
	{
		const newIcons: {[key: string]: string} = {}
		menu.forEach((item) =>
		{
			newIcons[item.path] = `${API_BASE_URL}/SvgImages/${item.path}`
		})
		setIcons(newIcons)
	}, [menu])

	useEffect(() =>
	{
		if (menu.length > 0) fetchIcons()
	}, [menu, fetchIcons])

	return (
		<Drawer
			variant="permanent"
			anchor="left"
			sx={{
				width: collapsed ? "3.83%" : "14%",
				flexShrink: 0,
				[`& .MuiDrawer-paper`]: {
					width: collapsed ? "3.83%" : "14%",
					boxSizing: "border-box",
					overflowX: "hidden",
				},
			}}
		>
			<Divider />
			<Box sx={{display: "flex", flexDirection: "column", height: "100%"}}>
				<List sx={{flexGrow: 1}}>
					{menu
						.filter((item) => item.mode.includes("d"))
						.map((item) =>
						{
							const toPath = `/dashboard/${item.path}`
							return (
								<ListItemButton
									key={item.path}
									component={Link}
									to={toPath}
									selected={currentPathDecoded.startsWith(toPath)}
									sx={{
										borderRadius: 2,
										mx: 1,
										my: 0.5,
										"&.Mui-selected": {
											backgroundColor: theme.palette.action.selected,
										},
									}}
								>
									<ListItemIcon>
										<img src={icons[item.path]} alt={item.path} width={24} height={24} style={{
											filter: isDarkTheme ? "invert(0%)" : "invert(100%)",
										}} />
									</ListItemIcon>
									{!collapsed && <ListItemText primary={item.path} sx={{color: theme.palette.text.primary}} />}
								</ListItemButton>
							)
						})}
					<Divider sx={{my: 1}} />
					<ListItemButton
						component={Link}
						to="/settings"
						selected={currentPathDecoded === "/settings"}
						sx={{
							borderRadius: 2,
							mx: 1,
							my: 0.5,
							"&.Mui-selected": {
								backgroundColor: theme.palette.action.selected,
							},
						}}
					>
						<ListItemIcon>
							<Settings sx={{color: theme.palette.text.primary}} />
						</ListItemIcon>
						{!collapsed && <ListItemText primary="Настройки" sx={{color: theme.palette.text.primary}} />}
					</ListItemButton>
					<ListItemButton
						sx={{
							borderRadius: 2,
							mx: 1,
							my: 0.5,
							"&.Mui-selected": {
								backgroundColor: theme.palette.action.selected,
							},
						}}
						onClick={(event) =>
						{
							if (collapsed)
							{
								event.stopPropagation()
								toggleTheme()
							}
						}}
					>
						<ListItemIcon sx={{color: theme.palette.text.primary}}>
							{isDarkTheme ? <Brightness7 /> : <Brightness4 />}
						</ListItemIcon>
						{!collapsed && <ListItemText primary="Темная тема" sx={{color: theme.palette.text.primary}} />}
						{!collapsed && (
							<Switch
								checked={isDarkTheme}
								onChange={(event) =>
								{
									event.stopPropagation()
									toggleTheme()
								}}
								sx={{
									"& .MuiSwitch-switchBase.Mui-checked": {
										color: theme.palette.primary.main,
									},
								}}
							/>
						)}
					</ListItemButton>
				</List>
				<Box sx={{padding: "16px 0", textAlign: "center"}}>
					<ListItemButton
						onClick={() => setCollapsed((prev) => !prev)}
						sx={{
							borderRadius: 2,
							mx: 1,
							my: 0.5,
							"&.Mui-selected": {
								backgroundColor: theme.palette.action.selected,
							},
						}}
					>
						<ListItemIcon sx={{color: theme.palette.text.primary}}>
							<Menu />
						</ListItemIcon>
						{!collapsed && <ListItemText primary="Свернуть" sx={{color: theme.palette.text.primary}} />}
					</ListItemButton>
				</Box>
			</Box>
		</Drawer>
	)
}

export default Sidebar
